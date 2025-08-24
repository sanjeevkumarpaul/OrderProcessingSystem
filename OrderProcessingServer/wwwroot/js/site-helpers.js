window.blazorDownload = (fileName, content) => {
    const data = new Blob([content], { type: 'text/csv;charset=utf-8;' });
    if (navigator.msSaveBlob) { // IE 10+
        navigator.msSaveBlob(data, fileName);
    } else {
        const link = document.createElement('a');
        const url = URL.createObjectURL(data);
        link.setAttribute('href', url);
        link.setAttribute('download', fileName);
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        URL.revokeObjectURL(url);
    }
};

window.showPopup = (btnId, popupId) => {
    try {
        const btn = document.getElementById(btnId);
        const popup = document.getElementById(popupId);
        if (!btn || !popup) return;

        // ensure popup is visible and on top
        popup.style.display = 'block';
        popup.style.zIndex = 999999;

        // position popup near button (below)
        const rect = btn.getBoundingClientRect();
        const popupRect = popup.getBoundingClientRect();
        let top = rect.bottom + 4 + window.scrollY;
        let left = rect.left + window.scrollX;

        // ensure it doesn't go off screen horizontally
        if (left + popupRect.width > window.innerWidth - 8) {
            left = Math.max(8, window.innerWidth - popupRect.width - 8);
        }

        popup.style.position = 'absolute';
        popup.style.top = top + 'px';
        popup.style.left = left + 'px';

        // bring into view
        popup.scrollIntoView({ block: 'nearest', inline: 'nearest' });
    }
    catch (e) {
        console.warn('showPopup failed', e);
    }
};

window.hidePopup = (popupId) => {
    try {
        const popup = document.getElementById(popupId);
        if (!popup) return;
        popup.style.display = 'none';
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
