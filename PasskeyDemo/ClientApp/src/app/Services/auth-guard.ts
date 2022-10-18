import {Injectable} from "@angular/core";
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree} from "@angular/router";
import {Observable} from "rxjs";
import {JsonWebTokenService} from "./json-web-token.service";

@Injectable()
export class AuthGuard implements CanActivate {

  constructor(
    private router: Router,
    private jsonWebTokenService: JsonWebTokenService) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    let isLoggedIn = this.jsonWebTokenService.IsLoggedIn();

    if (!isLoggedIn){
      this.router.navigateByUrl("/");
      return false;
    }

    return true;
  }
}
