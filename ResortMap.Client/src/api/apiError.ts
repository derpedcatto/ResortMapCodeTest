import type { ApiProblem } from "@/types/api";

export class ApiError extends Error {
  status: number;
  title: string;
  detail?: string;

  constructor(problem: ApiProblem) {
    super(problem.detail ?? problem.title);
    this.name = "ApiError";
    this.status = problem.status;
    this.title = problem.title;
    this.detail = problem.detail;
  }
}
