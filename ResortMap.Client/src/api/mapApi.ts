import type { ResortMapGrid } from "@/types/map";
import { apiRequest } from "./apiClient";

export function fetchMap(signal?: AbortSignal): Promise<ResortMapGrid> {
  return apiRequest<ResortMapGrid>("/api/map", { signal });
}
