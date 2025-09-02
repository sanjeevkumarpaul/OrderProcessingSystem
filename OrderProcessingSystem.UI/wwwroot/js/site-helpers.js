window.blazorDownload = (fileName, content) => {
    const data = new Blob([content], { type: 'text/csv;charset=utf-8;' });
    if (navigator.msSaveBlob) { // IE 10+
        navigator.msSaveBlob(data, fileName);
    } else {
        const url = URL.createObjectURL(data);
        const $link = $('<a>').attr({ href: url, download: fileName }).css('visibility', 'hidden').appendTo('body');
        $link[0].click();
        $link.remove();
        URL.revokeObjectURL(url);
    }
};

window.showPopup = (btnId, popupId, attempt) => {
    try {
        const MAX_ATTEMPTS = 12; // tolerate Blazor/layout timing when table is empty
        const RETRY_MS = 80;
        attempt = attempt || 0;

        const $btn = $('#' + $.escapeSelector(btnId));
        const btn = $btn.length ? $btn[0] : null;

        // Prefer a popup element that lives inside the same TH as the button
        let $popup = $();
        try {
            const $header = $btn.closest('th');
            if ($header && $header.length) {
                $popup = $header.find('#' + $.escapeSelector(popupId));
            }
        } catch (e) { $popup = $(); }
        if (!$popup || !$popup.length) $popup = $('#' + $.escapeSelector(popupId));

        if (!$btn.length || !$popup.length) {
            if (attempt < MAX_ATTEMPTS) {
                setTimeout(() => window.showPopup(btnId, popupId, attempt + 1), RETRY_MS);
            }
            return;
        }

        // move popup to body to avoid clipping
        const popup = $popup[0];
        if (!popup.dataset.movedToBody) {
            const $parent = $popup.parent();
            if ($parent && $parent.length) {
                const placeholderId = popupId + '-placeholder';
                const $placeholder = $('<div>').hide().attr('id', placeholderId);
                $popup.before($placeholder);
                popup.dataset.placeholderId = placeholderId;
                $('body').append($popup);
                popup.dataset.movedToBody = 'true';
            }
        }

        // ensure visible and on top
        $popup.css({ display: 'block', zIndex: 2147483000 });

        try {
            // inject CSS if needed
            if (!$('#grid-popup-css').length) {
                $('head').append('<link id="grid-popup-css" rel="stylesheet" href="/css/grid-popup.css">');
            }

            const $header = $btn.closest('th');
            const $anchorEl = ($header.length ? $header.find('.d-flex').first() : $btn).length ? ($header.length ? $header.find('.d-flex').first() : $btn) : $btn;

            let anchorRect = null;
            try {
                const anchorEl = $anchorEl.length ? $anchorEl[0] : $btn[0];
                anchorRect = anchorEl.getBoundingClientRect();
            } catch (e) {
                try {
                    const th = $btn.closest('th');
                    anchorRect = th.length ? th[0].getBoundingClientRect() : $btn[0].getBoundingClientRect();
                } catch (e2) {
                    anchorRect = { left: 8, right: 120, top: 8, bottom: 24, width: 112, height: 16 };
                }
            }

            if (!anchorRect || (anchorRect.width === 0 && anchorRect.height === 0)) {
                try {
                    const alt = $btn[0].getBoundingClientRect();
                    if (alt && (alt.width || alt.height)) anchorRect = alt;
                } catch (e) { }
            }

            // stage offscreen and measure
            $popup.css({ position: 'fixed', top: '-9999px', left: '-9999px', display: 'block' });
            let measured = popup.getBoundingClientRect();
            const minWidth = 200;
            if (measured.width < minWidth) {
                $popup.css('minWidth', minWidth + 'px');
                measured = popup.getBoundingClientRect();
            }
            const popupWidth = measured.width;
            const popupHeight = measured.height;

            // compute top anchored to header bottom
            let headerRect = null;
            try { headerRect = $header.length ? $header[0].getBoundingClientRect() : null; } catch (e) { headerRect = null; }
            let top = (headerRect ? headerRect.bottom : anchorRect.bottom) + 6;

            // ensure normal background
            $popup.addClass('bg-white').css({ backgroundColor: '', background: '' });

            // alignment
            let align = 'left';
            try {
                const headerEl = $header.length ? $header[0] : ($anchorEl.length ? $anchorEl[0] : null);
                if (headerEl) {
                    const $h = $(headerEl);
                    if ($h.hasClass('text-end')) align = 'right';
                    else {
                        const cs = window.getComputedStyle(headerEl);
                        if (cs && (cs.textAlign === 'right' || cs.textAlign === 'end')) align = 'right';
                    }
                }
            } catch (e) { }

            let left = (align === 'right') ? (anchorRect.right - popupWidth - 6) : (anchorRect.left + 6);
            const margin = 8;

            // prefer clamping to nearest grid/table container
            let containerRect = null;
            try {
                const $gridEl = $btn.closest('.generic-grid, .grid-wrapper, .table-responsive, table');
                if ($gridEl.length) containerRect = $gridEl[0].getBoundingClientRect();
            } catch (e) { containerRect = null; }

            const clampLeft = containerRect ? (containerRect.left + margin) : margin;
            const clampRight = containerRect ? (containerRect.right - margin) : (window.innerWidth - margin);
            const clampTop = containerRect ? (containerRect.top + margin) : margin;
            const clampBottom = containerRect ? (containerRect.bottom - margin) : (window.innerHeight - margin);

            if (left < clampLeft) left = clampLeft;
            if (left + popupWidth > clampRight) left = Math.max(clampLeft, clampRight - popupWidth);

            if (top + popupHeight > clampBottom) {
                top = (headerRect ? headerRect.top : anchorRect.top) - popupHeight - 6;
                $popup.addClass('flip-up');
            } else {
                $popup.removeClass('flip-up');
            }

            if (top < clampTop) top = clampTop;
            if (top + popupHeight > clampBottom) top = Math.max(clampTop, clampBottom - popupHeight);

            $popup.css({ top: Math.round(top) + 'px', left: Math.round(left) + 'px' });

            // caret classes
            if (align === 'right') $popup.addClass('caret-right'); else $popup.removeClass('caret-right');
            try {
                const pxLeft = Math.round(left);
                const headerCenter = headerRect ? (headerRect.left + headerRect.right) / 2 : (anchorRect.left + anchorRect.right) / 2;
                let caretX = Math.round(headerCenter - pxLeft);
                if (caretX < 12) caretX = 12;
                if (caretX > Math.round(popupWidth) - 12) caretX = Math.round(popupWidth) - 12;
                $popup.css('--caret-x', caretX + 'px');
            } catch (e) { }

            // outside-click handler using namespaced event so it can be removed per-popup
            try {
                const ns = 'mousedown.popup-' + popupId;
                // remove previous
                if (popup._outsideEventName) { $(document).off(popup._outsideEventName); popup._outsideEventName = null; }
                const handler = function (ev) {
                    try {
                        const target = ev.target;
                        if (!target) return;
                        if ($popup.is(target) || $.contains($popup[0], target) || ($btn && $.contains($btn[0], target))) return;
                        const $applyBtn = $popup.find('button.btn-primary').first();
                        if ($applyBtn && $applyBtn.length) { try { $applyBtn[0].click(); } catch (e) { } }
                        try { window.hidePopup(popupId); } catch (e) { }
                    } catch (e) { }
                };
                popup._outsideEventName = ns;
                $(document).on(ns, handler);
            } catch (e) { }

            try { $popup[0].scrollIntoView({ block: 'nearest', inline: 'nearest' }); } catch (e) { }
        }
        catch (posErr) {
            if (attempt < MAX_ATTEMPTS) {
                setTimeout(() => window.showPopup(btnId, popupId, attempt + 1), RETRY_MS);
                return;
            }
            // fallback simple placement
            try {
                const btnRect = $btn.length ? $btn[0].getBoundingClientRect() : { left: 8, right: 120, top: 8, bottom: 24, width: 112, height: 16 };
                $popup.css({ display: 'block', position: 'fixed', minWidth: '200px' });
                const measuredFallback = $popup[0].getBoundingClientRect();
                const fallbackWidth = measuredFallback.width || 200;
                const fallbackHeight = measuredFallback.height || 120;
                let fallbackLeft = (btnRect.left || 8) + 6;
                try {
                    const $headerEl = $btn.closest('th');
                    if ($headerEl.length && $headerEl.hasClass('text-end')) {
                        fallbackLeft = (btnRect.right || (btnRect.left + btnRect.width)) - fallbackWidth - 6;
                    }
                } catch (e) { }
                const margin = 8;
                if (fallbackLeft < margin) fallbackLeft = margin;
                if (fallbackLeft + fallbackWidth > window.innerWidth - margin) fallbackLeft = Math.max(margin, window.innerWidth - fallbackWidth - margin);
                let fallbackTop = (btnRect.bottom || 24) + 6;
                if (fallbackTop + fallbackHeight > window.innerHeight - margin) fallbackTop = (btnRect.top || 8) - fallbackHeight - 6;
                if (fallbackTop < margin) fallbackTop = margin;
                if (fallbackTop + fallbackHeight > window.innerHeight - margin) fallbackTop = Math.max(margin, window.innerHeight - fallbackHeight - margin);
                $popup.css({ left: Math.round(fallbackLeft) + 'px', top: Math.round(fallbackTop) + 'px' });
                try { $popup[0].scrollIntoView({ block: 'nearest', inline: 'nearest' }); } catch (e) { }
                return;
            } catch (fbErr) {
                console.warn('showPopup positioning failed after retries', posErr, fbErr);
            }
        }
    }
    catch (e) {
        console.warn('showPopup failed', e);
    }
};

window.hidePopup = (popupId) => {
    try {
        const $popup = $('#' + $.escapeSelector(popupId));
        if (!$popup.length) return;
        $popup.css({ display: 'none', position: '', top: '', left: '' });

        const popup = $popup[0];
        if (popup.dataset.movedToBody) {
            const placeholder = popup.dataset.placeholderId ? document.getElementById(popup.dataset.placeholderId) : null;
            if (placeholder && placeholder.parentElement) {
                placeholder.parentElement.insertBefore(popup, placeholder);
                placeholder.remove();
                delete popup.dataset.movedToBody;
                delete popup.dataset.placeholderId;
            } else {
                try { $popup.remove(); } catch (e) { }
            }
        }
        // remove namespaced outside handler if present
        try {
            if (popup._outsideEventName) { $(document).off(popup._outsideEventName); popup._outsideEventName = null; }
        } catch (e) { }
    }
    catch (e) {
        console.warn('hidePopup failed', e);
    }
};

// Open a new window with provided HTML and trigger print (user can choose Save as PDF)
window.openPrintWindow = (htmlContent) => {
    try {
        const win = window.open('', '_blank', 'width=900,height=700');
        if (!win) {
            console.warn('openPrintWindow: popup blocked');
            return;
        }
        // basic printable HTML; include Bootstrap from CDN for consistent styling if available
        const doc = win.document;
        doc.open();
        doc.write('<!doctype html><html><head><meta charset="utf-8"/>' +
            '<meta name="viewport" content="width=device-width, initial-scale=1"/>' +
            '<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css"/>' +
            '<title>Report Export</title></head><body class="p-3">');
        doc.write(htmlContent);
        doc.write('</body></html>');
        doc.close();

        // Wait briefly for resources to render then trigger print
        setTimeout(() => {
            try {
                win.focus();
                // In many browsers window.print() blocks until the print dialog closes.
                // Call print(), then close the print window immediately after the dialog returns
                win.print();
                try { win.close(); } catch (e) { /* ignore */ }
                // Fallback: ensure the window is closed after a short delay in case print() doesn't block in this browser
                setTimeout(() => { try { if (!win.closed) win.close(); } catch (e) { } }, 1500);
            } catch (e) {
                console.warn('print failed', e);
                try { if (!win.closed) win.close(); } catch (e) { }
            }
        }, 500);
    } catch (e) {
        console.warn('openPrintWindow failed', e);
    }
};

// Reads an element's innerHTML and opens the print window. Accepts an Element and optional includeBootstrap flag.
window.openPrintWindowFromElement = (el, includeBootstrap) => {
    try {
        if (!el) return;
        const html = el.innerHTML || '';
        let content = html;
        if (includeBootstrap === undefined || includeBootstrap === null) includeBootstrap = true;
        // If caller wants bootstrap injected, wrap content; the lower-level openPrintWindow already includes bootstrap tag when writing head.
        window.openPrintWindow(content);
    } catch (e) {
        console.warn('openPrintWindowFromElement failed', e);
    }
};

// load /Privacy page and show it inside the modal with id #privacyModal
window.showPrivacyModal = async (evt) => {
    try {
        // fetch the static privacy page HTML
        const res = await fetch('/Privacy');
        if (!res.ok) {
            console.warn('Failed to load privacy page', res.status);
            return;
        }
        const html = await res.text();

        // inject into modal body (strip any layout wrapper if present)
        const body = document.getElementById('privacyModalBody');
        if (!body) return;

        // Try to extract the inner markup if the returned page includes <body> or a main heading
        // Simple heuristic: find the first <h1> and return its container's following elements
        // Fallback: place full HTML
        let content = html;
        // If it's a razor page with a single <h1> and a paragraph, just keep from <h1>
        const h1Index = html.indexOf('<h1');
        if (h1Index >= 0) {
            content = html.substring(h1Index);
        }

        body.innerHTML = content;

        // show bootstrap modal
        const modalEl = document.getElementById('privacyModal');
        if (!modalEl) return;
        const modal = new bootstrap.Modal(modalEl);
        modal.show();
    }
    catch (e) {
        console.warn('showPrivacyModal failed', e);
    }
};
