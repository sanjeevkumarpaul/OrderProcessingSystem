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
