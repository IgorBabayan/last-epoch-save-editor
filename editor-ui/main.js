const { app, BrowserWindow, ipcMain } = require("electron");

const isDevMode = process.env.NODE_ENV === "development";
process.env["ELECTRON_DISABLE_SECURITY_WARNINGS"] = "true";

const createWindow = () => {
	const win = new BrowserWindow({
		width: isDevMode ? 1000 : 800,
		height: isDevMode ? 1000 : 600,
		title: "Last Epoch save editor",
		icon: `${__dirname}/dist/editor-ui/assets/images/icon.png`,
		frame: isDevMode,
		webPreferences: {
			nodeIntegration: true,
			contextIsolation: false,
			devTools: isDevMode,
			preload: `${__dirname}/dist/functions.js`,
		},
	});

	if (isDevMode) {
		win.webContents.openDevTools();
	}

	win.removeMenu();
	win.loadFile(`${__dirname}/dist/editor-ui/index.html`);

	ipcMain.on("maximize-window", () => console.log("maximize-window"));
	ipcMain.on("minimize-window", () => console.log("minimize-window"));
	ipcMain.on("close-window", () => console.log("close-window"));
};

app.whenReady().then(() => {
	createWindow();

	app.on("activate", () => {
		if (BrowserWindow.getAllWindows().length === 0) {
			createWindow();
		}
	});

	app.on("window-all-closed", () => {
		if (process.platform !== "darwin") {
			app.quit();
		}
	});
});
