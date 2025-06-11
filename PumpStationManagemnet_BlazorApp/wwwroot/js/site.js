// Biến để lưu trữ các biểu đồ đã tạo
window.chartInstances = {};

// Hàm vẽ biểu đồ tròn
window.drawPieChart = function(canvasId, data, options) {
    try {
        // Destroy existing chart if it exists
        if (window.chartInstances[canvasId]) {
            window.chartInstances[canvasId].destroy();
            delete window.chartInstances[canvasId];
        }

        const canvas = document.getElementById(canvasId);
        if (!canvas) {
            console.error('Canvas element not found:', canvasId);
            return;
        }

        const ctx = canvas.getContext('2d');
        
        // Tạo biểu đồ mới
        const chart = new Chart(ctx, {
            type: 'pie',
            data: data,
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false // Ẩn legend mặc định
                    },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                const label = context.label || '';
                                const value = context.parsed || 0;
                                return label + ': ' + value.toFixed(1) + '%';
                            }
                        }
                    }
                }
            }
        });

        // Lưu instance để có thể destroy sau này
        window.chartInstances[canvasId] = chart;
        
        console.log('Chart created successfully:', canvasId);
    } catch (error) {
        console.error('Error creating chart:', error);
    }
};

// Hàm để destroy tất cả biểu đồ (có thể dùng khi cần reset)
window.destroyAllCharts = function() {
    Object.keys(window.chartInstances).forEach(chartId => {
        if (window.chartInstances[chartId]) {
            window.chartInstances[chartId].destroy();
            delete window.chartInstances[chartId];
        }
    });
};

// Hàm kiểm tra Chart.js đã load chưa
window.isChartJsLoaded = function() {
    return typeof Chart !== 'undefined';
};

// Đợi Chart.js load xong trước khi vẽ biểu đồ
window.waitForChartJs = function() {
    return new Promise((resolve) => {
        if (typeof Chart !== 'undefined') {
            resolve();
            return;
        }
        
        const checkInterval = setInterval(() => {
            if (typeof Chart !== 'undefined') {
                clearInterval(checkInterval);
                resolve();
            }
        }, 100);
    });
};