import { useQuery } from "@tanstack/react-query";
import { fetchMap } from "@/api/mapApi";
import type { ResortMapDto } from "@/types/map";
import { decodeVisualGrid } from "@/lib/map/decodeVisualGrid";
import { decodeMapGrid } from "@/lib/map/decodeMapGrid";

const toVisualGrid = (dto: ResortMapDto) =>
  decodeVisualGrid(decodeMapGrid(dto.grid));

export function useResortMap() {
  return useQuery({
    queryKey: ["map"],
    queryFn: ({ signal }) => fetchMap(signal),
    staleTime: Infinity,
    select: toVisualGrid,
  });
}
