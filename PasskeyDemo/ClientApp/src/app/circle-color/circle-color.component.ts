import { Component, OnInit } from '@angular/core';
import {ColorService} from "../Services/color.service";
import {BrowserStorageService} from "../Services/browser-storage.service";
import {Constants} from "../Constants";
import {colors} from "@angular/cli/utilities/color";
import {firstValueFrom, Observable} from "rxjs";

@Component({
  selector: 'app-circle-color',
  templateUrl: './circle-color.component.html',
  styleUrls: ['./circle-color.component.css']
})
export class CircleColorComponent implements OnInit {

  color: string;

  constructor(
    private storage: BrowserStorageService,
    private colorService: ColorService) { }

  ngOnInit(): void {
    let userId = this.storage.GetValue<string>(Constants.UserId);
    this.colorService.GetColor(userId).subscribe(color => {
      console.log(color);
      if (color == undefined){
        this.color = "#621940"
        return;
      }
      this.color = color
    });
  }

  async OnColorChanged(): Promise<void> {
    console.log(this.color);

    let userId = this.storage.GetValue<string>(Constants.UserId);

    let colorSubstring = this.color.substring(1);

    await firstValueFrom(this.colorService.UpdateColor(userId, colorSubstring));

  }
}
