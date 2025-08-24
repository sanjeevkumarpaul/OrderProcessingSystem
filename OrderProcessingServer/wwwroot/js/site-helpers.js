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
