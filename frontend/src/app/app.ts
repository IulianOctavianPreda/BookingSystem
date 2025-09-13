import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {HlmCardModule} from "@ui/card"

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HlmCardModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('frontend-app');
}
