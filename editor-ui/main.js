const { app, BrowserWindow } = require("electron");
const isDevMode = process.env.NODE_ENV === "development";

const createWindow = () => {
  const win = new BrowserWindow({
    width: 800,
    height: 600,
    title: "Last Epoch save editor",
    icon: `${__dirname}/dist/editor-ui/assets/images/icon.png`,
    webPreferences: {
      nodeIntegration: true,
      contextIsolation: true,
      devTools: isDevMode,
    },
  });

  if (isDevMode) {
    win.webContents.openDevTools();
  }

  win.removeMenu();
  win.loadFile(`${__dirname}/dist/editor-ui/index.html`);
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
