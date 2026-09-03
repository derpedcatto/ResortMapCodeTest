import {
  useEffect,
  useRef,
  useState,
  type MouseEvent,
  type ReactNode,
} from "react";
import classNames from "classnames";
import {
  TransformComponent,
  TransformWrapper,
  useControls,
  useTransformEffect,
  type ReactZoomPanPinchRef,
} from "react-zoom-pan-pinch";
import { MapControls } from "@/components/molecules/MapControls/MapControls";
import { MapGrid } from "@/components/organisms/MapGrid/MapGrid";
import type { BookedCabanaSet, GridCoords, VisualTileGrid } from "@/types/map";
import styles from "./ResortMap.module.scss";

const INITIAL_SCALE = 0.4;
const MIN_SCALE = 0.4;
const MAX_SCALE = 2;
const ZOOM_STEP = 0.35;
const WHEEL_STEP = 0.35;

// Pointer travel in px above which a drag counts as a pan, not a tile click
const DRAG_THRESHOLD = 5;

type ResortMapProps = {
  tiles: VisualTileGrid;
  status?: ReactNode;
  bookedCabanas?: BookedCabanaSet;
  onSelectCabana?: (coords: GridCoords) => void;
  className?: string;
};

export function ResortMap({
  tiles,
  status,
  bookedCabanas,
  onSelectCabana,
  className,
}: ResortMapProps) {
  const pan = useRef({ x: 0, y: 0, dragged: false });
  const hasTiles = tiles.length > 0 && tiles[0].length > 0;
  const message = status ?? (hasTiles ? null : "The map is not available.");

  function handlePanningStart({ state }: ReactZoomPanPinchRef) {
    pan.current = { x: state.positionX, y: state.positionY, dragged: false };
  }

  function handlePanningStop({ state }: ReactZoomPanPinchRef) {
    const prev = pan.current;
    const dx = state.positionX - prev.x;
    const dy = state.positionY - prev.y;
    const distance = Math.hypot(dx, dy);

    pan.current.dragged = distance > DRAG_THRESHOLD;

    pan.current.x = state.positionX;
    pan.current.y = state.positionY;
  }

  // A drag that starts on a cabana must not book it on release
  function handleClickCapture(event: MouseEvent<HTMLDivElement>) {
    if (!pan.current.dragged) return;
    pan.current.dragged = false;
    event.stopPropagation();
  }

  return (
    <section className={classNames(styles.resortMap, className)}>
      <div className={styles.viewport}>
        {hasTiles && (
          <TransformWrapper
            initialScale={INITIAL_SCALE}
            minScale={MIN_SCALE}
            maxScale={MAX_SCALE}
            centerOnInit
            centerZoomedOut
            smooth={false}
            wheel={{ step: WHEEL_STEP }}
            doubleClick={{ disabled: true }}
            onPanningStart={handlePanningStart}
            onPanningStop={handlePanningStop}
          >
            <ZoomControls />

            <TransformComponent
              wrapperClass={styles.canvas}
              wrapperProps={{ onClickCapture: handleClickCapture }}
            >
              <MapGrid
                tiles={tiles}
                bookedCabanas={bookedCabanas}
                onSelectCabana={onSelectCabana}
              />
            </TransformComponent>
          </TransformWrapper>
        )}

        <div role="status" className={styles.status}>
          {message && <span className={styles.message}>{message}</span>}
        </div>
      </div>
    </section>
  );
}

function ZoomControls() {
  const { instance, zoomIn, zoomOut, resetTransform, centerView } =
    useControls();
  const [scale, setScale] = useState(instance.state.scale);
  const lastWidth = useRef(window.innerWidth);

  useTransformEffect(({ state }) => {
    setScale(state.scale);
  });

  // re-center map on viewport width change
  useEffect(() => {
    let timeoutId: ReturnType<typeof setTimeout>;

    function handleResize() {
      if (window.innerWidth === lastWidth.current) return;
      lastWidth.current = window.innerWidth;

      clearTimeout(timeoutId);

      timeoutId = setTimeout(() => {
        centerView(instance.state.scale);
      }, 200);
    }

    window.addEventListener("resize", handleResize);
    return () => {
      clearTimeout(timeoutId);
      window.removeEventListener("resize", handleResize);
    };
  }, [centerView, instance]);

  return (
    <MapControls
      onZoomIn={() => zoomIn(ZOOM_STEP)}
      onZoomOut={() => zoomOut(ZOOM_STEP)}
      onReset={() => resetTransform()}
      scale={scale}
      minScale={MIN_SCALE}
      maxScale={MAX_SCALE}
    />
  );
}
