import { Component, OnInit } from '@angular/core';
import {ColorService} from "../Services/color.service";
import {BrowserStorageService} from "../Services/browser-storage.service";
import {Constants} from "../Constants";
import {firstValueFrom, Observable} from "rxjs";

@Component({
  selector: 'app-circle-color',
  templateUrl: './circle-color.component.html',
  styleUrls: ['./circle-color.component.css']
})
export class CircleColorComponent implements OnInit {

  color: string;
  username: string;

  constructor(
    private storage: BrowserStorageService,
    private colorService: ColorService) { }

  ngOnInit(): void {
    this.username = this.storage.GetValue(Constants.Username);
    let userId = this.storage.GetValue<string>(Constants.UserId);
    this.colorService.GetColor(userId).subscribe(response => {

      if (!response.executedSuccessfully) {
        //Handle error
        return;
      }

      let color = response.result;

      if (color == undefined){
        this.color = "#621940"
        return;
      }
      this.color = color
    });
  }

  async OnColorChanged(): Promise<void> {
    let userId = this.storage.GetValue<string>(Constants.UserId);

    let colorSubstring = this.color.substring(1);

    let response = await firstValueFrom(this.colorService.UpdateColor(userId, colorSubstring));

    if (!response.executedSuccessfully){
      alert(response.message);
    }

  }
}
