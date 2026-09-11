import ky, { isHTTPError } from "ky";

type ProblemDetails = {
  title?: string;
  detail?: string;
  status?: number;
  errors?: Record<string, string[]>;
};

function toProblem(data: unknown): ProblemDetails | undefined {
  if (typeof data === "string") {
    try {
      data = JSON.parse(data);
    } catch {
      return undefined;
    }
  }

  return typeof data === "object" && data !== null
    ? (data as ProblemDetails)
    : undefined;
}

export const http = ky.create({
  retry: 0,
  hooks: {
    beforeError: [
      ({ error }) => {
        if (isHTTPError(error)) {
          const problem = toProblem(error.data);
          error.message = problem?.detail ?? problem?.title ?? error.message;
        }

        return error;
      },
    ],
  },
});
