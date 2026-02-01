window.razorpayInterop = {
    openCheckout: function (options, dotNetObject) {
        options.handler = function (response) {
            // Call C# method when payment succeeds
            dotNetObject.invokeMethodAsync('OnRazorpaySuccess',
                response.razorpay_order_id,
                response.razorpay_payment_id,
                response.razorpay_signature);
        };

        options.modal = {
            ondismiss: function () {
                // Optional: Handle modal closed without payment
            }
        };

        var rzp1 = new Razorpay(options);
        rzp1.on('payment.failed', function (response) {
            alert("Payment Failed: " + response.error.description);
        });
        rzp1.open();
    }
};