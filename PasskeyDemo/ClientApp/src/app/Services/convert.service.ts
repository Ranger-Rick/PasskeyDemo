import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
/**
 * Why this service is necessary is so unnecessary. Fido and the client implementations for WebAuthn I guess didn't consider
 * each other? And so they return things in formats that the other doesn't understand. This code is in the official Fido2
 * demo project. It is mind boggling that this garbage wasn't considered before-hand. Developers shouldn't have to do this
 * type of work
 */
export class ConvertService {

  constructor() { }

  CoerceToBase64Url(thing: any): string {
    // Array or ArrayBuffer to Uint8Array
    if (Array.isArray(thing)) {
      thing = Uint8Array.from(thing);
    }

    if (thing instanceof ArrayBuffer) {
      thing = new Uint8Array(thing);
    }

    // Uint8Array to base64
    if (thing instanceof Uint8Array) {
      var str = "";
      var len = thing.byteLength;

      for (var i = 0; i < len; i++) {
        str += String.fromCharCode(thing[i]);
      }
      thing = window.btoa(str);
    }

    if (typeof thing !== "string") {
      throw new Error("could not coerce to string");
    }

    // base64 to base64url
    // NOTE: "=" at the end of challenge is optional, strip it off here
    thing = thing.replace(/\+/g, "-").replace(/\//g, "_").replace(/=*$/g, "");

    return thing;
  }

  CoerceToBase64(challenge: string): Uint8Array {
    let replacedChallenge = challenge.replace(/-/g, "+").replace(/_/g, "/");
    let output = Uint8Array.from(atob(replacedChallenge), c => c.charCodeAt(0));
    return output;
  }

}
