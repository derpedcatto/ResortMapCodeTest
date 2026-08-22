import type { ResortMapDto } from "@/types/map";
import { apiRequest } from "./apiClient";

export function fetchMap(signal?: AbortSignal): Promise<ResortMapDto> {
  return apiRequest<ResortMapDto>("/api/map", { signal });
}
