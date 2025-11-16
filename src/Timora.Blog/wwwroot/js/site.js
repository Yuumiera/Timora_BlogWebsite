// Theme management
document.addEventListener('DOMContentLoaded', function () {
    const storageKey = 'theme';
    const root = document.documentElement;
    const saved = localStorage.getItem(storageKey) || 'light';
    
    // Apply saved theme
    root.setAttribute('data-theme', saved);
    
    // Wait a bit for buttons to be rendered
    setTimeout(function() {
        updateThemeButtons(saved);
        
        // Theme toggle buttons
        const themeButtons = document.querySelectorAll('.theme-btn');
        themeButtons.forEach(btn => {
            btn.addEventListener('click', function () {
                const theme = this.getAttribute('data-theme');
                root.setAttribute('data-theme', theme);
                localStorage.setItem(storageKey, theme);
                updateThemeButtons(theme);
                showToast('Tema değiştirildi', 'Tema başarıyla ' + (theme === 'light' ? 'açık' : theme === 'dark' ? 'koyu' : 'okuma') + ' moduna geçirildi.', 'success');
            });
        });
    }, 100);
    
    function updateThemeButtons(activeTheme) {
        const themeButtons = document.querySelectorAll('.theme-btn');
        themeButtons.forEach(btn => {
            const btnTheme = btn.getAttribute('data-theme');
            if (btnTheme === activeTheme) {
                btn.classList.add('active');
            } else {
                btn.classList.remove('active');
            }
        });
    }
});

// ============================================
// HCI PRINCIPLES - TOAST NOTIFICATIONS
// ============================================

/**
 * Show a toast notification
 * @param {string} title - Toast title
 * @param {string} message - Toast message
 * @param {string} type - Toast type: 'success', 'error', 'info', 'warning'
 * @param {number} duration - Auto-hide duration in ms (0 = no auto-hide)
 */
function showToast(title, message, type = 'info', duration = 5000) {
    // Create toast container if it doesn't exist
    let container = document.querySelector('.toast-container');
    if (!container) {
        container = document.createElement('div');
        container.className = 'toast-container';
        document.body.appendChild(container);
    }

    // Create toast element
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.setAttribute('role', 'alert');
    toast.setAttribute('aria-live', type === 'error' ? 'assertive' : 'polite');
    toast.setAttribute('aria-atomic', 'true');

    // Icons for different types
    const icons = {
        success: '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path><polyline points="22 4 12 14.01 9 11.01"></polyline></svg>',
        error: '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="8" x2="12" y2="12"></line><line x1="12" y1="16" x2="12.01" y2="16"></line></svg>',
        info: '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="16" x2="12" y2="12"></line><line x1="12" y1="8" x2="12.01" y2="8"></line></svg>',
        warning: '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"></path><line x1="12" y1="9" x2="12" y2="13"></line><line x1="12" y1="17" x2="12.01" y2="17"></line></svg>'
    };

    toast.innerHTML = `
        <div class="toast-icon">${icons[type] || icons.info}</div>
        <div class="toast-content">
            <div class="toast-title">${title}</div>
            <p class="toast-message">${message}</p>
        </div>
        <button type="button" class="toast-close" aria-label="Kapat">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <line x1="18" y1="6" x2="6" y2="18"></line>
                <line x1="6" y1="6" x2="18" y2="18"></line>
            </svg>
        </button>
    `;

    // Add close functionality
    const closeBtn = toast.querySelector('.toast-close');
    const closeToast = () => {
        toast.classList.add('hiding');
        setTimeout(() => {
            toast.remove();
            if (container.children.length === 0) {
                container.remove();
            }
        }, 300);
    };

    closeBtn.addEventListener('click', closeToast);

    // Auto-hide if duration is set
    if (duration > 0) {
        setTimeout(closeToast, duration);
    }

    // Add to container
    container.appendChild(toast);

    // Focus management for accessibility
    closeBtn.focus();
}

// ============================================
// HCI PRINCIPLES - CONFIRMATION DIALOG
// ============================================

/**
 * Show a confirmation dialog
 * @param {string} title - Dialog title
 * @param {string} message - Dialog message
 * @param {Function} onConfirm - Callback when confirmed
 * @param {Function} onCancel - Optional callback when cancelled
 */
function showConfirmation(title, message, onConfirm, onCancel = null) {
    // Create dialog overlay
    const dialog = document.createElement('div');
    dialog.className = 'confirmation-dialog';
    dialog.setAttribute('role', 'dialog');
    dialog.setAttribute('aria-modal', 'true');
    dialog.setAttribute('aria-labelledby', 'confirmation-title');
    dialog.setAttribute('aria-describedby', 'confirmation-message');

    dialog.innerHTML = `
        <div class="confirmation-dialog-content">
            <h2 id="confirmation-title" class="confirmation-dialog-title">${title}</h2>
            <p id="confirmation-message" class="confirmation-dialog-message">${message}</p>
            <div class="confirmation-dialog-actions">
                <button type="button" class="btn btn-outline-success" data-action="cancel">İptal</button>
                <button type="button" class="btn btn-success" data-action="confirm">Onayla</button>
            </div>
        </div>
    `;

    const closeDialog = (confirmed) => {
        dialog.classList.add('hiding');
        setTimeout(() => {
            dialog.remove();
            // Restore focus to trigger element
            if (document.activeElement && document.activeElement.blur) {
                document.activeElement.blur();
            }
        }, 200);

        if (confirmed && onConfirm) {
            onConfirm();
        } else if (!confirmed && onCancel) {
            onCancel();
        }
    };

    // Event listeners
    const confirmBtn = dialog.querySelector('[data-action="confirm"]');
    const cancelBtn = dialog.querySelector('[data-action="cancel"]');

    confirmBtn.addEventListener('click', () => closeDialog(true));
    cancelBtn.addEventListener('click', () => closeDialog(false));

    // Close on overlay click
    dialog.addEventListener('click', (e) => {
        if (e.target === dialog) {
            closeDialog(false);
        }
    });

    // Keyboard support
    dialog.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            closeDialog(false);
        } else if (e.key === 'Enter' && e.target === confirmBtn) {
            closeDialog(true);
        }
    });

    // Add to body
    document.body.appendChild(dialog);

    // Focus management
    cancelBtn.focus();

    // Trap focus within dialog
    const focusableElements = dialog.querySelectorAll('button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])');
    const firstElement = focusableElements[0];
    const lastElement = focusableElements[focusableElements.length - 1];

    dialog.addEventListener('keydown', (e) => {
        if (e.key === 'Tab') {
            if (e.shiftKey && document.activeElement === firstElement) {
                e.preventDefault();
                lastElement.focus();
            } else if (!e.shiftKey && document.activeElement === lastElement) {
                e.preventDefault();
                firstElement.focus();
            }
        }
    });
}

// ============================================
// HCI PRINCIPLES - LOADING STATES
// ============================================

/**
 * Show loading state on a button
 * @param {HTMLElement} button - Button element
 */
function setButtonLoading(button, loading = true) {
    if (loading) {
        button.disabled = true;
        button.classList.add('btn-loading');
        button.setAttribute('aria-busy', 'true');
    } else {
        button.disabled = false;
        button.classList.remove('btn-loading');
        button.removeAttribute('aria-busy');
    }
}

/**
 * Show progress indicator
 */
function showProgress() {
    let indicator = document.querySelector('.progress-indicator');
    if (!indicator) {
        indicator = document.createElement('div');
        indicator.className = 'progress-indicator';
        indicator.innerHTML = '<div class="progress-indicator-bar"></div>';
        document.body.appendChild(indicator);
    }
    indicator.style.display = 'block';
}

/**
 * Hide progress indicator
 */
function hideProgress() {
    const indicator = document.querySelector('.progress-indicator');
    if (indicator) {
        indicator.style.display = 'none';
    }
}

// ============================================
// HCI PRINCIPLES - FORM ENHANCEMENTS
// ============================================

// Auto-add loading states to forms
document.addEventListener('DOMContentLoaded', function() {
    // Add loading state to form submissions
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            const submitBtn = form.querySelector('button[type="submit"], input[type="submit"]');
            if (submitBtn && !submitBtn.disabled) {
                setButtonLoading(submitBtn, true);
                showProgress();
            }
        });
    });

    // Enhanced form validation feedback
    const inputs = document.querySelectorAll('input, select, textarea');
    inputs.forEach(input => {
        input.addEventListener('blur', function() {
            if (this.checkValidity()) {
                this.classList.remove('is-invalid');
                this.classList.add('is-valid');
            } else {
                this.classList.remove('is-valid');
                this.classList.add('is-invalid');
            }
        });

        input.addEventListener('input', function() {
            if (this.classList.contains('is-invalid') && this.checkValidity()) {
                this.classList.remove('is-invalid');
                this.classList.add('is-valid');
            }
        });
    });
});

// ============================================
// HCI PRINCIPLES - KEYBOARD SHORTCUTS
// ============================================

document.addEventListener('keydown', function(e) {
    // Ctrl/Cmd + K to focus search (if exists)
    if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
        const searchInput = document.querySelector('input[type="search"], input[placeholder*="ara"], input[placeholder*="Ara"]');
        if (searchInput) {
            e.preventDefault();
            searchInput.focus();
        }
    }

    // Escape to close modals/dropdowns
    if (e.key === 'Escape') {
        const openDropdowns = document.querySelectorAll('.dropdown-menu.show');
        openDropdowns.forEach(dropdown => {
            const toggle = document.querySelector(`[data-bs-toggle="dropdown"][aria-expanded="true"]`);
            if (toggle) {
                toggle.click();
            }
        });
    }
});

// ============================================
// HCI PRINCIPLES - ACCESSIBILITY ENHANCEMENTS
// ============================================

// Announce dynamic content changes to screen readers
function announceToScreenReader(message) {
    const announcement = document.createElement('div');
    announcement.setAttribute('role', 'status');
    announcement.setAttribute('aria-live', 'polite');
    announcement.setAttribute('aria-atomic', 'true');
    announcement.className = 'sr-only';
    announcement.textContent = message;
    document.body.appendChild(announcement);
    setTimeout(() => {
        announcement.remove();
    }, 1000);
}

// Export functions for global use
window.showToast = showToast;
window.showConfirmation = showConfirmation;
window.setButtonLoading = setButtonLoading;
window.showProgress = showProgress;
window.hideProgress = hideProgress;
window.announceToScreenReader = announceToScreenReader;
