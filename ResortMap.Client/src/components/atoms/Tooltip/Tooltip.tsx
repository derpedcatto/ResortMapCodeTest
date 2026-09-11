import classNames from "classnames";
import styles from "./Tooltip.module.scss";

type TooltipProps = {
  label: string;
  className?: string;
};

export function Tooltip({ label, className }: TooltipProps) {
  return (
    <div aria-hidden="true" className={classNames(styles.tooltip, className)}>
      {label}
    </div>
  );
}
