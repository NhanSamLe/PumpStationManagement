window.downloadFileFromStream = (fileName, byteArray) => {
    try {
        const blob = new Blob([new Uint8Array(byteArray)], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        const downloadUrl = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = downloadUrl;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        a.remove();
        window.URL.revokeObjectURL(downloadUrl);
    } catch (error) {
        console.error('Tải file thất bại:', error);
        alert('Không thể tải file Excel: ' + error.message);
    }
};