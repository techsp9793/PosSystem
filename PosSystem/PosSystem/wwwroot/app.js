window.printReceipt = (receiptHtml) => {
    const printWindow = window.open('', '', 'width=400,height=600');
    printWindow.document.write(receiptHtml);
    printWindow.document.close();
    printWindow.focus();

    // Wait for content to load then print
    setTimeout(() => {
        printWindow.print();
        printWindow.close();
    }, 500);
};