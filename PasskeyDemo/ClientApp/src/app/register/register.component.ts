import {Component, OnInit} from '@angular/core';
import {TestingAuthenticationService} from "../Services/authentication.service";
import {ConvertService} from "../Services/convert.service";
import {firstValueFrom} from "rxjs";
import {PersistentPropertiesService} from "../Services/persistent-properties.service";
import {Router} from "@angular/router";
import {IApiResponse} from "../Interfaces/IApiResponse";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  username: string = "";

  constructor(
    private route: Router,
    private authService: TestingAuthenticationService,
    private convertService: ConvertService,
    private persistentStorage: PersistentPropertiesService
  ) { }

  ngOnInit(): void {
  }

  async Register(): Promise<void>{
    let isUsernameAvailable = await firstValueFrom(this.authService.IsUsernameAvailable(this.username));

    if (!isUsernameAvailable.executedSuccessfully || !isUsernameAvailable.result) {
      this.HandleError(isUsernameAvailable);
      return;
    }

    let credentialOptions = await firstValueFrom(this.authService.GetCredentialOptions(this.username));

    if (!credentialOptions.executedSuccessfully) {
      this.HandleError(credentialOptions);
      return;
    }

    let credential = await this.MakeCredential(credentialOptions.result);

    credentialOptions.result.challenge = this.convertService.CoerceToBase64Url(credentialOptions.result.challenge);
    credentialOptions.result.user.id = this.convertService.CoerceToBase64Url(credentialOptions.result.user.id);
    let rawId = this.convertService.CoerceToBase64Url(credential.rawId);
    let attestationObject = this.convertService.CoerceToBase64Url(credential.response.attestationObject);
    let clientDataJson = this.convertService.CoerceToBase64Url(credential.response.clientDataJSON);

    let requestBody = {
      id: credential.id,
      rawId: rawId,
      attestationObject: attestationObject,
      clientDataJSON: clientDataJson,
      options: credentialOptions.result
    }

    let response = await firstValueFrom(this.authService.MakeCredential(requestBody));

    if (!response.executedSuccessfully){
      alert("Unable to register your account");
      return;
    }

    this.persistentStorage.SetPersistentProperties(response.result);
    await this.route.navigateByUrl("/circle");
  }

  MakeCredential(credentialOptions: any): Promise<any> {
    try {

      let enc = new TextEncoder();
      let userIdAsArrayBuffer = enc.encode(credentialOptions.user.id);

      credentialOptions.challenge = Uint8Array.from(credentialOptions.challenge);
      credentialOptions.user.id = userIdAsArrayBuffer;

      return navigator.credentials.create({
        publicKey: credentialOptions
      });
    }
    catch {
      return new Promise<any>(() => {});
    }
  }

  HandleError(response: IApiResponse) {
    alert(response.message);
  }

}
