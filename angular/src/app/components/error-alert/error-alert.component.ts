import { Component, Input } from '@angular/core';


@Component({
  selector: 'app-error-alert',
  standalone: true,
  imports: [],
  templateUrl: './error-alert.component.html',
  styleUrl: './error-alert.component.css'
})
export class ErrorAlertComponent {

  @Input() message!: string;
  @Input() errors: string[] = [];

  close(item: string): void {
    document.getElementById(item)?.remove();
  }

  onFadeOutDelete(item: string): void {
    let element = document.getElementById(item);
    if(!element) {
      return;
    }
    const opacity = window.getComputedStyle(element).opacity;
    if (opacity === '0') {
      element.remove();
    }
  }
}
