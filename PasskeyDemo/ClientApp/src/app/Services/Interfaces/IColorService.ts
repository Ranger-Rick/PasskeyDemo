import {Observable} from "rxjs";
import {IApiResponse, ITypedApiResponse} from "../../Interfaces/IApiResponse";

export interface IColorService {
  GetColor(userId: string): Observable<ITypedApiResponse<string>>;
  UpdateColor(userId: string, color: string): Observable<IApiResponse>;
}
