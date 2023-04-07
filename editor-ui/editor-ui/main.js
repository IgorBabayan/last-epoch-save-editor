const { app, BrowserWindow } = require("electron");

let win;
const isMac = process.platform === "darwin";
const isDev = process.env.NODE_ENV === "development";

function createWindow() {
  process.env["ELECTRON_DISABLE_SECURITY_WARNINGS"] = "true";
  win = new BrowserWindow({
    title: "Last Epoch save editor",
    width: isDev ? 1000 : 800,
    height: isDev ? 1000 : 600,
    icon: `${__dirname}/dist/editor-ui/assets/icon.png`,
    webPreferences: {
      nodeIntegration: true
    }
  });

  win.removeMenu();
  if (isDev)
    win.webContents.openDevTools();

  win.loadFile(`${__dirname}/dist/editor-ui/index.html`);
  win.on("closed", () => {
    win = null;
  });
}

app.on("ready", createWindow);

app.on("window-all-closed", () => {
  if (!isMac)
    app.quit();
});

app.on("activate", () => {
  if (win === null || win === undefined)
    createWindow();
});