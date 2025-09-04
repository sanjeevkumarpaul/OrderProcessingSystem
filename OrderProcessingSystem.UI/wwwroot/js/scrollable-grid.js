// Scrollable Grid Enhancements
window.scrollableGridManager = {
    init: function() {
        this.setupScrollIndicators();
        this.setupSmoothScrolling();
    },

    setupScrollIndicators: function() {
        const containers = document.querySelectorAll('.scrollable-grid-container');
        
        containers.forEach(container => {
            this.updateScrollIndicators(container);
            
            container.addEventListener('scroll', () => {
                this.updateScrollIndicators(container);
            });
        });
    },

    updateScrollIndicators: function(container) {
        const { scrollTop, scrollHeight, clientHeight } = container;
        
        // Add/remove top scroll indicator
        if (scrollTop > 10) {
            container.classList.add('has-scroll-top');
        } else {
            container.classList.remove('has-scroll-top');
        }
        
        // Add/remove bottom scroll indicator  
        if (scrollTop < scrollHeight - clientHeight - 10) {
            container.classList.add('has-scroll-bottom');
        } else {
            container.classList.remove('has-scroll-bottom');
        }
    },

    setupSmoothScrolling: function() {
        // Enhance keyboard navigation
        document.addEventListener('keydown', (e) => {
            const activeGrid = document.querySelector('.scrollable-grid-container:hover');
            if (!activeGrid) return;

            switch(e.key) {
                case 'Home':
                    if (e.ctrlKey) {
                        e.preventDefault();
                        activeGrid.scrollTo({ top: 0, behavior: 'smooth' });
                    }
                    break;
                case 'End':
                    if (e.ctrlKey) {
                        e.preventDefault();
                        activeGrid.scrollTo({ top: activeGrid.scrollHeight, behavior: 'smooth' });
                    }
                    break;
                case 'PageUp':
                    e.preventDefault();
                    activeGrid.scrollBy({ top: -activeGrid.clientHeight * 0.8, behavior: 'smooth' });
                    break;
                case 'PageDown':
                    e.preventDefault();
                    activeGrid.scrollBy({ top: activeGrid.clientHeight * 0.8, behavior: 'smooth' });
                    break;
            }
        });
    },

    // Utility method to scroll to top of grid
    scrollToTop: function(containerId) {
        const container = document.getElementById(containerId);
        if (container) {
            container.scrollTo({ top: 0, behavior: 'smooth' });
        }
    },

    // Utility method to highlight new data in grid
    highlightNewRows: function(containerId, rowCount = 1) {
        const container = document.getElementById(containerId);
        if (!container) return;

        const rows = container.querySelectorAll('tbody tr');
        for (let i = 0; i < Math.min(rowCount, rows.length); i++) {
            rows[i].classList.add('highlight-new-row');
            setTimeout(() => {
                rows[i].classList.remove('highlight-new-row');
            }, 2000);
        }
    }
};

// Initialize when DOM is loaded
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => window.scrollableGridManager.init());
} else {
    window.scrollableGridManager.init();
}

// Re-initialize when Blazor updates the page
window.addEventListener('blazor:enhanced:navigated', () => {
    setTimeout(() => window.scrollableGridManager.init(), 100);
});
