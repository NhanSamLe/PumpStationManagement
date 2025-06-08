window.drawPieChart = (canvasId, chartData, chartOptions) => {
    console.log("Đang cố gắng vẽ biểu đồ cho canvasId:", canvasId); // Log debug
    const ctx = document.getElementById(canvasId)?.getContext('2d');

    if (!ctx) {
        console.error(`Không tìm thấy canvas với id '${canvasId}'`);
        return;
    }

    // Hủy biểu đồ hiện có nếu tồn tại
    if (window[canvasId]) {
        window[canvasId].destroy();
    }

    // Tạo biểu đồ tròn Chart.js mới với chú thích tùy chỉnh
    window[canvasId] = new Chart(ctx, {
        type: 'pie',
        data: chartData,
        options: {
            ...chartOptions,
            plugins: {
                ...chartOptions.plugins,
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            const label = context.label || '';
                            const value = context.parsed || 0;
                            return `${label}: ${value}%`;
                        }
                    }
                }
            }
        }
    });
};