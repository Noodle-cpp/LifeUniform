(() => {
  const MAX_COLORS = 8;
  const rowsRoot = document.getElementById('colorRows');
  const addBtn = document.getElementById('addColorBtn');
  const matrixRoot = document.getElementById('colorSizeMatrix');

  let initialStock = new Set();
  let hasInitialData = false;
  try {
    const raw = matrixRoot?.dataset.initial;
    if (raw) {
      const arr = JSON.parse(raw);
      arr.forEach((k) => initialStock.add(String(k)));
      hasInitialData = arr.length > 0;
    }
  } catch (_) { /* ignore */ }

  function reindexRows() {
    if (!rowsRoot) return;
    [...rowsRoot.querySelectorAll('[data-color-row]')].forEach((row, i) => {
      row.querySelectorAll('input').forEach((input) => {
        const name = input.getAttribute('name') || '';
        const match = name.match(/^Vm\.Colors\[\d+](\..+)$/);
        if (match) input.setAttribute('name', `Vm.Colors[${i}]${match[1]}`);
        const id = input.getAttribute('id') || '';
        const idMatch = id.match(/^Vm_Colors_\d+__(.+)$/);
        if (idMatch) input.setAttribute('id', `Vm_Colors_${i}__${idMatch[1]}`);
      });
    });
    if (addBtn) addBtn.disabled = rowsRoot.querySelectorAll('[data-color-row]').length >= MAX_COLORS;
    rebuildMatrix();
  }

  function clearRow(row) {
    row.querySelectorAll('input').forEach((input) => {
      if (input.type === 'color') input.value = '#cccccc';
      else input.value = '';
    });
  }

  function selectedSizes() {
    return [...document.querySelectorAll('.js-product-size:checked')].map((el) => ({
      id: el.value,
      label: el.getAttribute('data-size-label') || el.value
    }));
  }

  function colorNames() {
    if (!rowsRoot) return [];
    return [...rowsRoot.querySelectorAll('[data-color-name]')]
      .map((el) => (el.value || '').trim())
      .filter(Boolean);
  }

  function stockKey(color, sizeId) {
    return `${color}||${sizeId}`;
  }

  function rebuildMatrix() {
    if (!matrixRoot) return;
    const colors = colorNames();
    const sizes = selectedSizes();

    // Remember current checks before rebuild
    matrixRoot.querySelectorAll('input[name="InStockColorSizeKeys"]:checked').forEach((el) => {
      initialStock.add(el.value);
    });

    if (colors.length === 0 || sizes.length === 0) {
      matrixRoot.innerHTML = '<div class="text-muted small">Сначала укажите цвета и размеры товара.</div>';
      return;
    }

    const table = document.createElement('div');
    table.className = 'admin-stock-table';

    const head = document.createElement('div');
    head.className = 'admin-stock-row admin-stock-row--head';
    head.innerHTML = `<div class="admin-stock-cell admin-stock-cell--label">Цвет</div>` +
      sizes.map((s) => `<div class="admin-stock-cell">${s.label}</div>`).join('');
    table.appendChild(head);

    colors.forEach((color) => {
      const row = document.createElement('div');
      row.className = 'admin-stock-row';
      row.innerHTML = `<div class="admin-stock-cell admin-stock-cell--label">${color}</div>` +
        sizes.map((s) => {
          const key = stockKey(color, s.id);
          const checked = (!hasInitialData || initialStock.has(key)) ? 'checked' : '';
          return `<div class="admin-stock-cell">
            <label class="admin-size-pill admin-size-pill--sm">
              <input type="checkbox" name="InStockColorSizeKeys" value="${key}" ${checked} />
              <span>${s.label}</span>
            </label>
          </div>`;
        }).join('');
      table.appendChild(row);
    });

    matrixRoot.innerHTML = '';
    matrixRoot.appendChild(table);
  }

  rowsRoot?.addEventListener('click', (e) => {
    const btn = e.target.closest('[data-remove-color]');
    if (!btn) return;
    const row = btn.closest('[data-color-row]');
    if (!row) return;

    const rows = rowsRoot.querySelectorAll('[data-color-row]');
    if (rows.length <= 1) {
      clearRow(row);
      reindexRows();
      return;
    }

    row.remove();
    reindexRows();
  });

  rowsRoot?.addEventListener('input', (e) => {
    if (e.target.matches('[data-color-name]')) rebuildMatrix();
  });

  addBtn?.addEventListener('click', () => {
    if (!rowsRoot) return;
    const rows = rowsRoot.querySelectorAll('[data-color-row]');
    if (rows.length >= MAX_COLORS) return;

    const template = rows[rows.length - 1];
    const clone = template.cloneNode(true);
    clearRow(clone);
    rowsRoot.appendChild(clone);
    reindexRows();
  });

  document.querySelectorAll('.js-product-size').forEach((el) => {
    el.addEventListener('change', rebuildMatrix);
  });

  reindexRows();
})();

