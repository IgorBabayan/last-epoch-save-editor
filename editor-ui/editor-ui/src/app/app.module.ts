import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { WindowHeaderComponent } from './window-header/window-header.component';

@NgModule({
  declarations: [AppComponent, WindowHeaderComponent],
  imports: [BrowserModule, AppRoutingModule],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {
  static TITLE = 'Last Epoch save editor';
  static ICON = './dist/editor-ui/assets/icon.png';
}
