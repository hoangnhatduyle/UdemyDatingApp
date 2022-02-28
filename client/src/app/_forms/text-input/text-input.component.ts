import { Component, Input, OnInit } from '@angular/core';
import { ControlValueAccessor } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})
export class TextInputComponent implements ControlValueAccessor {
  @Input() label: string;
  @Input() type = 'text';

  constructor() { }
  
  writeValue(obj: any): void {
    
  }

  registerOnChange(fn: any): void {
    
  }

  registerOnTouched(fn: any): void {
    
  }

  ngOnInit(): void {
  }

}
