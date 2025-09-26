/** Generic API wrapper from server, handle a list of responses. */
export interface ApiResponse<T> {
    statusCode: number;
    message: string;
    data: T[];
}

/** Api validatio error and contains validation error information. */
export interface ApiValidationError extends ApiBaseError {
    validationErrors?: string[];
}

/** Api problem response model. All errors different from validation. e.g. 500 */
export interface ApiError extends ApiBaseError {
    details: string;
}

/** Base Api error model. */
export interface ApiBaseError {
    statusCode: number;
    message?: string;
}

/** Master error api model. */
export type ApiErrorResponse = ApiValidationError | ApiError | null;

/** API response wrapper, handles a singular data response. */
export interface ApiSingleResponse<T> {
    statusCode: number;
    message: string;
    data: T;
}

/** common status response codes. */
export const StatusCode = {
  Okay: 200,
  Accepted: 201,
  BadRequest: 400,
  Unauthorized: 401,
  Forbidden: 403,
  NotFound: 404,
  ServerError: 500,
} 
