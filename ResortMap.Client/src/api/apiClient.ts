import { ApiError, type ApiProblem } from "./apiError";

type RequestOptions = {
  method?: string;
  body?: unknown;
  signal?: AbortSignal;
};

const NETWORK_ERROR: ApiProblem = {
  title: "Network error",
  status: 0,
  detail: "Could not reach the server",
};

function isApiProblem(value: unknown): value is ApiProblem {
  return (
    typeof value === "object" &&
    value !== null &&
    typeof (value as ApiProblem).title === "string" &&
    typeof (value as ApiProblem).status === "number"
  );
}

async function readProblem(res: Response): Promise<ApiProblem> {
  try {
    const body = await res.json();

    if (isApiProblem(body)) {
      return body;
    }
  } catch {
    // body empty / no json
  }

  return {
    title: "Request failed",
    status: res.status,
    detail: `The server responded with status ${res.status}`,
  };
}

export async function apiRequest<T>(
  path: string,
  { method = "GET", body, signal }: RequestOptions = {},
): Promise<T> {
  const hasBody = body !== undefined;
  let res: Response;

  try {
    res = await fetch(path, {
      method,
      signal,
      headers: hasBody ? { "Content-Type": "application/json" } : undefined,
      body: hasBody ? JSON.stringify(body) : undefined,
    });
  } catch (error) {
    if (error instanceof DOMException && error.name === "AbortError") {
      throw error;
    }

    throw new ApiError(NETWORK_ERROR);
  }

  if (!res.ok) {
    throw new ApiError(await readProblem(res));
  }

  const text = await res.text();

  if (!text) {
    return undefined as T;
  }

  try {
    return JSON.parse(text) as T;
  } catch {
    throw new ApiError({
      title: "Invalid response",
      status: res.status,
      detail: "The server returned an invalid response",
    });
  }
}
