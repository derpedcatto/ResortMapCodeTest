import { useQuery } from "@tanstack/react-query";
import { fetchMap } from "@/api/mapApi";

export function useResortMap() {
  return useQuery({
    queryKey: ["map"],
    queryFn: ({ signal }) => fetchMap(signal),
    staleTime: Infinity,
  });
}
