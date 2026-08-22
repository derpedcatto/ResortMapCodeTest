import arrowCornerSquare from "@/assets/arrowCornerSquare.png";
import arrowCrossing from "@/assets/arrowCrossing.png";
import arrowEnd from "@/assets/arrowEnd.png";
import arrowSplit from "@/assets/arrowSplit.png";
import arrowStraight from "@/assets/arrowStraight.png";
import cabana from "@/assets/cabana.png";
import houseChimney from "@/assets/houseChimney.png";
import parchmentBasic from "@/assets/parchmentBasic.png";
import pool from "@/assets/pool.png";
import textureWater from "@/assets/textureWater.png";
import type { SpriteAsset, SpriteKey } from "@/types/sprites";

export const SPRITES: Record<SpriteKey, SpriteAsset> = {
  cabana: { src: cabana, alt: "Cabana" },
  pool: { src: pool, alt: "Pool" },
  chalet: { src: houseChimney, alt: "Chalet" },
  pathStraight: { src: arrowStraight, alt: "Path" },
  pathCorner: { src: arrowCornerSquare, alt: "Path corner" },
  pathEnd: { src: arrowEnd, alt: "Path end" },
  pathSplit: { src: arrowSplit, alt: "Path junction" },
  pathCrossing: { src: arrowCrossing, alt: "Path crossing" },
  background: { src: parchmentBasic, alt: "" },
  water: { src: textureWater, alt: "" },
  empty: { src: "", alt: "" },
};
