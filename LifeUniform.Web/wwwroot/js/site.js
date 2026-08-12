// Password visibility + auth modal helpers
(() => {
  document.addEventListener('click', (e) => {
    const btn = e.target.closest('[data-toggle-password]');
    if (!btn) return;
    const input = document.getElementById(btn.getAttribute('data-toggle-password'));
    if (!input) return;
    const show = input.type === 'password';
    input.type = show ? 'text' : 'password';
    const icon = btn.querySelector('i');
    if (icon) {
      icon.classList.toggle('bi-eye', !show);
      icon.classList.toggle('bi-eye-slash', show);
    }
  });

  // Smooth switch between login/register modals
  document.querySelectorAll('[data-bs-target="#loginModal"], [data-bs-target="#registerModal"]').forEach((el) => {
    el.addEventListener('click', (e) => {
      const targetId = el.getAttribute('data-bs-target');
      const current = el.closest('.modal');
      if (!current || !targetId || !window.bootstrap) return;
      e.preventDefault();
      const currentModal = bootstrap.Modal.getInstance(current);
      const nextEl = document.querySelector(targetId);
      if (!nextEl) return;
      current.addEventListener('hidden.bs.modal', () => {
        bootstrap.Modal.getOrCreateInstance(nextEl).show();
      }, { once: true });
      currentModal?.hide();
    });
  });
})();

// Favorites: AJAX toggle without full page reload
(() => {
  const inflight = new WeakSet();

  function applyFavoriteState(form, isFavorite) {
    const btn = form.querySelector('.js-favorite-btn');
    const icon = form.querySelector('.js-favorite-icon');
    const label = form.querySelector('.js-favorite-label');
    if (!btn) return;

    btn.classList.toggle('is-active', isFavorite);
    btn.setAttribute('aria-pressed', isFavorite ? 'true' : 'false');
    btn.dataset.favorite = isFavorite ? 'true' : 'false';
    btn.title = isFavorite ? 'В избранном' : 'В избранное';

    if (icon) {
      icon.classList.toggle('bi-heart-fill', isFavorite);
      icon.classList.toggle('bi-heart', !isFavorite);
    }

    if (label) {
      const on = btn.dataset.labelOn || 'В избранном';
      const off = btn.dataset.labelOff || 'В избранное';
      label.textContent = isFavorite ? on : off;
    }
  }

  document.addEventListener('submit', async (e) => {
    const form = e.target.closest('.js-favorite-form');
    if (!form) return;
    e.preventDefault();

    if (inflight.has(form)) return;
    inflight.add(form);

    const btn = form.querySelector('.js-favorite-btn');
    if (btn) btn.disabled = true;

    try {
      const res = await fetch(form.action, {
        method: 'POST',
        body: new FormData(form),
        headers: {
          Accept: 'application/json',
          'X-Requested-With': 'XMLHttpRequest'
        },
        credentials: 'same-origin'
      });

      if (res.status === 401) {
        // Keep heart empty for guests
        applyFavoriteState(form, false);
        const login = document.getElementById('loginModal');
        if (login && window.bootstrap) {
          bootstrap.Modal.getOrCreateInstance(login).show();
        } else {
          window.location.href = '/Account/Auth?returnUrl=' + encodeURIComponent(window.location.pathname);
        }
        return;
      }

      if (!res.ok) throw new Error('favorite toggle failed');
      const data = await res.json();
      if (data && typeof data.isFavorite === 'boolean') {
        applyFavoriteState(form, data.isFavorite);
      }
    } catch (_) {
      // Fallback: classic post if AJAX fails
      form.classList.remove('js-favorite-form');
      form.submit();
    } finally {
      inflight.delete(form);
      if (btn) btn.disabled = false;
    }
  });
})();

// Product card: preview color-linked image on swatch hover
(() => {
  const applySrc = (img, src) => {
    if (!img || !src) return;
    img.src = src;
    if (/^https?:\/\//i.test(src)) img.setAttribute('referrerpolicy', 'no-referrer');
    else img.removeAttribute('referrerpolicy');
  };

  document.addEventListener('mouseover', (e) => {
    const dot = e.target.closest('.js-color-preview');
    if (!dot) return;
    const card = dot.closest('.product-card');
    const img = card?.querySelector('.js-product-card-img');
    const src = dot.getAttribute('data-image');
    if (img && src) applySrc(img, src);
  });

  document.addEventListener('mouseout', (e) => {
    const dot = e.target.closest('.js-color-preview');
    if (!dot) return;
    const related = e.relatedTarget;
    if (related && (dot === related || dot.contains(related))) return;
    const card = dot.closest('.product-card');
    const img = card?.querySelector('.js-product-card-img');
    const fallback = img?.getAttribute('data-default-src');
    if (img && fallback) applySrc(img, fallback);
  });
})();
