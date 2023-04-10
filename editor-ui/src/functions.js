const { ipcRenderer } = require('electron');

window.addEventListener("DOMContentLoaded", () => {
	document.querySelector(".button.minimize").addEventListener("click", () => ipcRenderer.send("minimize-window"));
	document.querySelector(".button.maximize").addEventListener("click", () => ipcRenderer.send("maximize-window"));
	document.querySelector(".button.close").addEventListener("click", () => ipcRenderer.send("close-window"));
});
