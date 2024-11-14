import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-success-alert',
  standalone: true,
  imports: [],
  templateUrl: './success-alert.component.html',
  styleUrl: './success-alert.component.css'
})
export class SuccessAlertComponent {
  @Input() message!: string;

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
