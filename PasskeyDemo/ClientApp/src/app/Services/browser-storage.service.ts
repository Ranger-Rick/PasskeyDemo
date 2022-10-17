import { Injectable } from '@angular/core';
import {IPersistentStorageRepository} from "./Interfaces/IPersistentStorageRepository";

@Injectable({
  providedIn: 'root'
})
export class BrowserStorageService implements IPersistentStorageRepository{

  constructor() { }

  GetValue<Type>(key: string): Type {
    let value = localStorage.getItem(key);
    return value as unknown as Type;
  }

  SetValue(key: string, value: string): void {
    localStorage.setItem(key, value);
  }
}
