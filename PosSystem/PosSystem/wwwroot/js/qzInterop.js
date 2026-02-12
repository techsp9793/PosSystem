window.qzInterop = {
    // 1. Connect to QZ Tray
    ensureConnection: async function () {
        if (!qz.websocket.isActive()) {
            try {
                await qz.websocket.connect();
                console.log("Connected to QZ Tray");
            } catch (err) {
                console.error("QZ Connection Error:", err);
                alert("QZ Tray is not running! Please start it.");
                throw err;
            }
        }
    },

    // 2. Send the "Kick" Command
    openDrawer: async function (printerName) {
        try {
            await this.ensureConnection();

            var config = qz.configs.create(printerName);

            // ESC/POS Command to open drawer (Standard Pin 2)
            // Hex: 1B 70 00 19 FA
            var data = ['\x1B' + '\x70' + '\x00' + '\x19' + '\xFA'];

            await qz.print(config, data);
            return "Drawer Triggered";
        } catch (err) {
            console.error(err);
            return "Error: " + err;
        }
    }
};