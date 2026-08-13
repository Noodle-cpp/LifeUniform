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
    const wasFavorite = btn?.dataset.favorite === 'true';
    // Optimistic UI: heart flips immediately
    applyFavoriteState(form, !wasFavorite);
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
        applyFavoriteState(form, false);
        return;
      }

      if (!res.ok) throw new Error('favorite toggle failed');
      const data = await res.json();
      if (data && typeof data.isFavorite === 'boolean') {
        applyFavoriteState(form, data.isFavorite);
      }
    } catch (_) {
      applyFavoriteState(form, wasFavorite);
    } finally {
      inflight.delete(form);
      if (btn) btn.disabled = false;
    }
  });
})();

// Product card: color swatch changes image in place (click selects, hover previews)
(() => {
  const applySrc = (img, src) => {
    if (!img || !src) return;
    img.src = src;
    if (/^https?:\/\//i.test(src)) img.setAttribute('referrerpolicy', 'no-referrer');
    else img.removeAttribute('referrerpolicy');
  };

  const committedSrc = (img) => img?.getAttribute('data-active-src') || img?.getAttribute('data-default-src');

  document.addEventListener('click', (e) => {
    const dot = e.target.closest('.js-color-preview');
    if (!dot) return;
    e.preventDefault();
    const card = dot.closest('.product-card');
    if (!card) return;
    card.querySelectorAll('.js-color-preview').forEach((d) => d.classList.remove('is-selected'));
    dot.classList.add('is-selected');
    const img = card.querySelector('.js-product-card-img');
    const src = dot.getAttribute('data-image') || committedSrc(img);
    if (img && src) {
      applySrc(img, src);
      img.setAttribute('data-active-src', src);
    }
  });

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
    const fallback = committedSrc(img);
    if (img && fallback) applySrc(img, fallback);
  });

  document.addEventListener('click', (e) => {
    const pill = e.target.closest('.js-size-pill');
    if (!pill) return;
    const row = pill.closest('.size-pills__row');
    if (!row) return;
    row.querySelectorAll('.js-size-pill').forEach((p) => p.classList.remove('is-selected'));
    pill.classList.add('is-selected');
  });
})();

// Horizontal product / review rails
(() => {
  document.querySelectorAll('[data-rail]').forEach((rail) => {
    const track = rail.querySelector('.product-rail__track');
    const prev = rail.querySelector('.product-rail__nav--prev');
    const next = rail.querySelector('.product-rail__nav--next');
    if (!track) return;
    const amount = () => {
      const item = track.querySelector('.product-rail__item, .promo-slide, .client-photo-card, .catalog-products__item');
      if (!item) return Math.max(track.clientWidth, 260);
      const gap = parseFloat(getComputedStyle(track).gap || '0') || 0;
      return Math.round(item.getBoundingClientRect().width + gap);
    };
    const atStart = () => track.scrollLeft <= 8;
    const atEnd = () => track.scrollLeft + track.clientWidth >= track.scrollWidth - 16;
    const goNext = () => {
      if (track.scrollWidth <= track.clientWidth + 12) return;
      if (atEnd()) track.scrollTo({ left: 0, behavior: 'smooth' });
      else track.scrollBy({ left: amount(), behavior: 'smooth' });
    };
    const goPrev = () => {
      if (track.scrollWidth <= track.clientWidth + 12) return;
      if (atStart()) track.scrollTo({ left: track.scrollWidth, behavior: 'smooth' });
      else track.scrollBy({ left: -amount(), behavior: 'smooth' });
    };
    prev?.addEventListener('click', goPrev);
    next?.addEventListener('click', goNext);

    if (!rail.hasAttribute('data-rail-auto')) return;
    if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) return;

    let timer = 0;
    const start = () => {
      window.clearInterval(timer);
      timer = window.setInterval(goNext, 5500);
    };
    const stop = () => window.clearInterval(timer);
    start();
    rail.addEventListener('mouseenter', stop);
    rail.addEventListener('mouseleave', start);
    track.addEventListener('pointerdown', stop);
    track.addEventListener('pointerup', start);
  });
})();

// Search overlay (not a catalog page)
(() => {
  const overlay = document.getElementById('searchOverlay');
  const form = document.getElementById('searchOverlayForm');
  const input = document.getElementById('searchOverlayInput');
  const clearBtn = document.getElementById('searchOverlayClear');
  const meta = document.getElementById('searchOverlayMeta');
  const results = document.getElementById('searchOverlayResults');
  const empty = document.getElementById('searchOverlayEmpty');
  const progress = document.getElementById('searchOverlayProgress');
  const moreBtn = document.getElementById('searchOverlayMore');
  const pager = document.getElementById('searchOverlayPager');
  if (!overlay || !input || !results) return;

  let timer = 0;
  let page = 1;
  let lastQuery = '';
  let requestId = 0;

  const escapeHtml = (value) => String(value ?? '')
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;');

  const highlight = (text, query) => {
    const safe = escapeHtml(text);
    const q = (query || '').trim();
    if (q.length < 2) return safe;
    const re = new RegExp(`(${q.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')})`, 'gi');
    return safe.replace(re, '<mark>$1</mark>');
  };

  const formatPrice = (n) => `${new Intl.NumberFormat('ru-RU').format(n)} ₽`;

  const setLoading = (isLoading) => {
    if (!progress) return;
    progress.hidden = !isLoading;
    overlay.classList.toggle('is-loading', isLoading);
  };

  const setEmpty = (isEmpty) => {
    if (empty) empty.hidden = !isEmpty;
  };

  const renderItem = (item, query) => {
    const img = escapeHtml(item.previewImageUrl);
    const href = `/Catalog/Details/${encodeURIComponent(item.slug)}`;
    const snippet = item.snippet
      ? `<div class="search-result__snippet">${highlight(item.snippet, query)}</div>`
      : '';
    const badge = item.discountPercent
      ? `<span class="search-result__badge">−<span class="js-count-up" data-value="${item.discountPercent}" data-suffix="%">0%</span></span>`
      : '';
    const oldPrice = item.oldPrice
      ? `<span class="search-result__price-old">${formatPrice(item.oldPrice)}</span>`
      : '';
    const priceValue = Number(item.price);
    const priceHtml = item.oldPrice
      ? `<span class="search-result__price-current"><span class="js-count-up" data-value="${priceValue}" data-suffix=" ₽">0 ₽</span></span>`
      : `<span class="search-result__price-current">${formatPrice(item.price)}</span>`;
    return `<a class="search-result" href="${href}">
      <div class="search-result__media">
        <img src="${img}" alt="">
        ${badge}
      </div>
      <div>
        <div class="search-result__title">${highlight(item.name, query)}</div>
        ${snippet}
        <div class="search-result__price">
          ${oldPrice}
          ${priceHtml}
        </div>
      </div>
    </a>`;
  };

  const renderPager = (data) => {
    if (!pager) return;
    if (data.totalPages <= 1) {
      pager.innerHTML = '';
      return;
    }
    let html = '';
    for (let i = 1; i <= data.totalPages; i += 1) {
      html += `<button type="button" class="search-page ${i === data.page ? 'is-active' : ''}" data-page="${i}">${i}</button>`;
    }
    if (data.page < data.totalPages) {
      html += `<button type="button" class="search-page" data-page="${data.page + 1}">›</button>`;
    }
    pager.innerHTML = html;
  };

  const syncScrollLock = () => {
    const lock = overlay.classList.contains('is-open') && overlay.classList.contains('has-query');
    document.body.classList.toggle('search-open', lock);
  };

  const setQueryState = (hasQuery) => {
    overlay.classList.toggle('has-query', hasQuery);
    syncScrollLock();
  };

  const clearResultsUi = () => {
    results.innerHTML = '';
    if (meta) meta.textContent = '';
    if (moreBtn) moreBtn.hidden = true;
    if (pager) pager.innerHTML = '';
    setEmpty(false);
    setLoading(false);
  };

  const load = async (query, pageNumber, append) => {
    lastQuery = query;
    page = pageNumber;
    const hasQuery = query.length > 0;
    setQueryState(hasQuery);
    if (!hasQuery) {
      clearResultsUi();
      return;
    }

    const currentRequest = ++requestId;
    setLoading(true);
    if (!append) {
      setEmpty(false);
      if (moreBtn) moreBtn.hidden = true;
    }

    try {
      const url = `/Catalog/Search?q=${encodeURIComponent(query)}&pageNumber=${pageNumber}`;
      const res = await fetch(url, { headers: { Accept: 'application/json' } });
      if (!res.ok) return;
      const data = await res.json();
      if (currentRequest !== requestId) return;

      const total = data.totalCount || 0;
      const isEmpty = total === 0;
      setEmpty(isEmpty && !append);
      if (meta) {
        meta.textContent = isEmpty
          ? ''
          : `${total} результат${total === 1 ? '' : total < 5 ? 'а' : 'ов'} по запросу «${query}»`;
      }
      const html = (data.items || []).map((item) => renderItem(item, query)).join('');
      if (append) results.insertAdjacentHTML('beforeend', html);
      else results.innerHTML = html;
      if (window.luCountUp) {
        window.luCountUp.observe(results.querySelectorAll('.js-count-up'));
      }
      if (moreBtn) moreBtn.hidden = !(data.page < data.totalPages);
      renderPager(data);
    } finally {
      if (currentRequest === requestId) setLoading(false);
    }
  };

  const open = () => {
    overlay.hidden = false;
    overlay.offsetHeight;
    requestAnimationFrame(() => {
      overlay.classList.add('is-open');
      syncScrollLock();
      input.focus();
    });
  };

  const close = () => {
    overlay.classList.remove('is-open');
    overlay.classList.remove('has-query');
    overlay.classList.remove('is-loading');
    document.body.classList.remove('search-open');
    setLoading(false);
    window.setTimeout(() => {
      if (!overlay.classList.contains('is-open')) overlay.hidden = true;
    }, 480);
  };

  document.querySelectorAll('.js-search-open').forEach((btn) => {
    btn.addEventListener('click', open);
  });
  document.querySelectorAll('.js-search-close').forEach((btn) => {
    btn.addEventListener('click', close);
  });
  overlay.addEventListener('click', (e) => {
    if (e.target === overlay) close();
  });
  document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && overlay.classList.contains('is-open')) close();
  });

  form?.addEventListener('submit', (e) => {
    e.preventDefault();
    load(input.value.trim(), 1, false);
  });

  input.addEventListener('input', () => {
    const q = input.value.trim();
    if (clearBtn) clearBtn.hidden = q.length === 0;
    window.clearTimeout(timer);
    timer = window.setTimeout(() => {
      if (q.length === 0) {
        clearResultsUi();
        setQueryState(false);
        return;
      }
      load(q, 1, false);
    }, 220);
  });

  clearBtn?.addEventListener('click', () => {
    input.value = '';
    input.dispatchEvent(new Event('input'));
    input.focus();
  });

  moreBtn?.addEventListener('click', () => load(lastQuery, page + 1, true));

  pager?.addEventListener('click', (e) => {
    const btn = e.target.closest('[data-page]');
    if (!btn) return;
    load(lastQuery, Number(btn.getAttribute('data-page')), false);
    overlay.querySelector('.search-overlay__body')?.scrollTo({ top: 0 });
  });
})();

// Hero typewriter: cycle adjectives only
(() => {
  const el = document.querySelector('.js-typewriter');
  if (!el) return;

  const words = (el.getAttribute('data-words') || el.getAttribute('data-text') || '')
    .split(',')
    .map((w) => w.trim().toLowerCase())
    .filter(Boolean);
  if (words.length === 0) return;

  const title = el.closest('.hero-banner__title');
  const syncAria = (word) => {
    if (!title) return;
    title.setAttribute('aria-label', `${word} медицинская одежда`);
  };

  if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
    el.textContent = words[0];
    el.classList.add('is-done');
    syncAria(words[0]);
    return;
  }

  const typeDelay = 42;
  const deleteDelay = 28;
  const holdFullMs = 2600;
  const holdEmptyMs = 320;
  let wordIndex = 0;
  let i = 0;
  let deleting = false;

  const current = () => words[wordIndex % words.length];

  const tick = () => {
    const word = current();
    if (!deleting) {
      i += 1;
      el.textContent = word.slice(0, i);
      if (i < word.length) {
        window.setTimeout(tick, typeDelay);
        return;
      }
      syncAria(word);
      window.setTimeout(() => {
        deleting = true;
        tick();
      }, holdFullMs);
      return;
    }

    i -= 1;
    el.textContent = word.slice(0, Math.max(0, i));
    if (i > 0) {
      window.setTimeout(tick, deleteDelay);
      return;
    }
    deleting = false;
    wordIndex = (wordIndex + 1) % words.length;
    window.setTimeout(tick, holdEmptyMs);
  };

  window.setTimeout(tick, 280);
})();

// Fade-in on scroll / load
(() => {
  const nodes = Array.from(document.querySelectorAll('.js-reveal'));
  if (nodes.length === 0) return;

  if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
    nodes.forEach((n) => n.classList.add('is-visible'));
    return;
  }

  const show = (el) => {
    const delay = Number(el.getAttribute('data-reveal-delay') || 0);
    window.setTimeout(() => el.classList.add('is-visible'), delay);
  };

  if (!('IntersectionObserver' in window)) {
    nodes.forEach(show);
    return;
  }

  const io = new IntersectionObserver((entries) => {
    entries.forEach((entry) => {
      if (!entry.isIntersecting) return;
      show(entry.target);
      io.unobserve(entry.target);
    });
  }, { threshold: 0.12, rootMargin: '0px 0px -40px 0px' });

  nodes.forEach((n) => io.observe(n));
})();

// Sticky header frosted glass on scroll
(() => {
  const nav = document.querySelector('.site-nav');
  if (!nav) return;
  const sync = () => {
    nav.classList.toggle('is-scrolled', window.scrollY > 12);
  };
  sync();
  window.addEventListener('scroll', sync, { passive: true });
})();

// Count-up for discounts / promo prices
(() => {
  const reduceMotion = () => window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  const format = (value, decimals) => {
    const n = decimals > 0 ? value.toFixed(decimals) : String(Math.round(value));
    return new Intl.NumberFormat('ru-RU').format(decimals > 0 ? Number(n) : Math.round(value));
  };

  const run = (el) => {
    if (!el || el.dataset.counted === '1') return;
    const reveal = el.closest('.js-reveal');
    if (reveal && !reveal.classList.contains('is-visible')) return;

    el.dataset.counted = '1';
    const raw = el.getAttribute('data-value') || '0';
    const target = Number(raw);
    const suffix = el.getAttribute('data-suffix') || '';
    const decimals = raw.includes('.') ? 2 : 0;
    if (!Number.isFinite(target) || reduceMotion()) {
      el.textContent = `${format(Number.isFinite(target) ? target : 0, decimals)}${suffix}`;
      return;
    }

    const duration = 900;
    const start = performance.now();
    const step = (now) => {
      const t = Math.min(1, (now - start) / duration);
      const eased = 1 - Math.pow(1 - t, 3);
      el.textContent = `${format(target * eased, decimals)}${suffix}`;
      if (t < 1) requestAnimationFrame(step);
      else el.textContent = `${format(target, decimals)}${suffix}`;
    };
    requestAnimationFrame(step);
  };

  const observe = (nodes) => {
    const list = Array.from(nodes || []);
    if (list.length === 0) return;

    if (!('IntersectionObserver' in window)) {
      list.forEach(run);
      return;
    }

    const io = new IntersectionObserver((entries) => {
      entries.forEach((entry) => {
        if (!entry.isIntersecting) return;
        run(entry.target);
        if (entry.target.dataset.counted === '1') io.unobserve(entry.target);
      });
    }, { threshold: 0.2, rootMargin: '0px 0px -8% 0px' });

    list.forEach((n) => io.observe(n));
  };

  // When fade-in sections appear, start nested counters
  document.querySelectorAll('.js-reveal').forEach((reveal) => {
    const kick = () => observe(reveal.querySelectorAll('.js-count-up'));
    if (reveal.classList.contains('is-visible')) {
      kick();
      return;
    }
    const mo = new MutationObserver(() => {
      if (!reveal.classList.contains('is-visible')) return;
      kick();
      mo.disconnect();
    });
    mo.observe(reveal, { attributes: true, attributeFilter: ['class'] });
  });

  observe(document.querySelectorAll('.js-count-up'));
  window.luCountUp = { run, observe };
})();

// Lazy image loading spinner / fade-in
(() => {
  const done = (img) => {
    const frame = img.closest('.media-frame');
    if (frame) frame.classList.remove('is-loading');
  };

  document.querySelectorAll('.js-lazy-img').forEach((img) => {
    if (img.complete && img.naturalWidth > 0) {
      done(img);
      return;
    }
    img.addEventListener('load', () => done(img), { once: true });
    img.addEventListener('error', () => done(img), { once: true });
  });
})();
