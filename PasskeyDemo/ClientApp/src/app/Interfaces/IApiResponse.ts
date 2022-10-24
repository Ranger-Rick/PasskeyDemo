export interface IApiResponse {
  executedSuccessfully: boolean;
  message: string;
}

export interface ITypedApiResponse<Type> extends IApiResponse {
  result: Type;
}
