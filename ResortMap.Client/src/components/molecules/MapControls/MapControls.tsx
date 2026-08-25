import { IconButton } from "@/components/atoms/IconButton/IconButton";
import { FaPlus, FaMinus } from "react-icons/fa";
import { FaArrowRotateLeft } from "react-icons/fa6";
import classNames from "classnames";
import styles from "./MapControls.module.scss";

type MapControlsProps = {
  onZoomIn: () => void;
  onZoomOut: () => void;
  onReset: () => void;
  scale: number;
  minScale: number;
  maxScale: number;
  className?: string;
};

export function MapControls({
  onZoomIn,
  onZoomOut,
  onReset,
  scale,
  minScale,
  maxScale,
  className,
}: MapControlsProps) {
  return (
    <div
      role="group"
      aria-label="Map Controls"
      className={classNames(styles.controls, className)}
    >
      <IconButton
        icon={FaPlus}
        label="Zoom In"
        onClick={onZoomIn}
        disabled={scale >= maxScale}
      />
      <IconButton
        icon={FaMinus}
        label="Zoom Out"
        onClick={onZoomOut}
        disabled={scale <= minScale}
      />
      <IconButton
        icon={FaArrowRotateLeft}
        label="Reset View"
        onClick={onReset}
        disabled={scale === 1}
      />
    </div>
  );
}
