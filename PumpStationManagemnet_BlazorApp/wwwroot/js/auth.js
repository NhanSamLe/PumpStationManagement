// Toast notification functions
window.showToast = function (type, message) {
    // Tạo element toast
    const toast = document.createElement('div');
    toast.className = `toast-notification toast-${type}`;

    // Icon theo loại
    const icon = type === 'success' ? 'fas fa-check-circle' :
        type === 'error' ? 'fas fa-exclamation-circle' :
            'fas fa-info-circle';

    toast.innerHTML = `
        <div class="toast-content">
            <i class="${icon}"></i>
            <span>${message}</span>
        </div>
        <button class="toast-close" onclick="this.parentElement.remove()">
            <i class="fas fa-times"></i>
        </button>
    `;

    // Thêm styles nếu chưa có
    if (!document.querySelector('#toast-styles')) {
        const styles = document.createElement('style');
        styles.id = 'toast-styles';
        styles.textContent = `
            .toast-notification {
                position: fixed;
                top: 20px;
                right: 20px;
                min-width: 300px;
                padding: 15px;
                border-radius: 8px;
                color: white;
                font-weight: 500;
                z-index: 9999;
                display: flex;
                align-items: center;
                justify-content: space-between;
                box-shadow: 0 4px 12px rgba(0,0,0,0.15);
                animation: slideInRight 0.3s ease-out;
            }
            
            .toast-success {
                background: linear-gradient(135deg, #28a745, #20c997);
            }
            
            .toast-error {
                background: linear-gradient(135deg, #dc3545, #e74c3c);
            }
            
            .toast-info {
                background: linear-gradient(135deg, #17a2b8, #3498db);
            }
            
            .toast-content {
                display: flex;
                align-items: center;
                gap: 10px;
            }
            
            .toast-close {
                background: none;
                border: none;
                color: white;
                cursor: pointer;
                padding: 5px;
                opacity: 0.8;
            }
            
            .toast-close:hover {
                opacity: 1;
            }
            
            @keyframes slideInRight {
                from {
                    transform: translateX(100%);
                    opacity: 0;
                }
                to {
                    transform: translateX(0);
                    opacity: 1;
                }
            }
        `;
        document.head.appendChild(styles);
    }

    // Thêm toast vào body
    document.body.appendChild(toast);

    // Tự động xóa sau 5 giây
    setTimeout(() => {
        if (toast.parentElement) {
            toast.style.animation = 'slideInRight 0.3s ease-out reverse';
            setTimeout(() => toast.remove(), 300);
        }
    }, 5000);
};

// Utility functions
window.authUtils = {
    clearStorage: function () {
        localStorage.removeItem('userId');
        localStorage.removeItem('username');
        localStorage.removeItem('userFullName');
        localStorage.removeItem('userRole');
    },

    setFocus: function (elementId) {
        setTimeout(() => {
            const element = document.getElementById(elementId);
            if (element) {
                element.focus();
            }
        }, 100);
    }
};