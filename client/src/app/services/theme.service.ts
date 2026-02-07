import { Injectable, signal, effect } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  isDarkMode = signal<boolean>(false);

  constructor() {
    // Load theme preference from localStorage
    const savedTheme = localStorage.getItem('theme');
    this.isDarkMode.set(savedTheme === 'dark');

    // Apply theme on init
    this.applyTheme();

    // Watch for changes and apply theme
    effect(() => {
      this.applyTheme();
      localStorage.setItem('theme', this.isDarkMode() ? 'dark' : 'light');
    });
  }

  toggleTheme() {
    this.isDarkMode.update(value => !value);
  }

  private applyTheme() {
    const body = document.body;
    if (this.isDarkMode()) {
      body.classList.add('dark-theme');
      body.classList.remove('light-theme');
    } else {
      body.classList.add('light-theme');
      body.classList.remove('dark-theme');
    }
  }
}