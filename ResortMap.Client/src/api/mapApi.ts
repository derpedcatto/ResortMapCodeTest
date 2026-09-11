import type { ResortMapDto } from "@/types/map";
import { http } from "./http";

export function fetchMap(signal?: AbortSignal): Promise<ResortMapDto> {
  return http.get("/api/map", { signal }).json<ResortMapDto>();
}
