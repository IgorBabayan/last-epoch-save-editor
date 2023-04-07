import { Component } from '@angular/core';
import { AppModule } from '../app.module';

@Component({
  selector: 'app-window-header',
  templateUrl: './window-header.component.html',
  styleUrls: ['./_window-header.component.scss'],
})
export class WindowHeaderComponent {
  title = AppModule.TITLE;
  icon = AppModule.ICON;
}
