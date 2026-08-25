import classNames from "classnames";
import type { IconType } from "react-icons";
import styles from "./IconButton.module.scss";

type IconButtonProps = {
  icon: IconType;
  label: string;
  onClick?: () => void;
  disabled?: boolean;
  className?: string;
};

export function IconButton({
  icon: Icon,
  label,
  onClick,
  disabled = false,
  className,
}: IconButtonProps) {
  function handleClick() {
    if (disabled) return;
    onClick?.();
  }

  return (
    <button
      type="button"
      onClick={handleClick}
      aria-disabled={disabled || undefined}
      className={classNames(styles.button, className)}
    >
      <Icon aria-hidden="true" focusable="false" />
      <span className={styles.label}>{label}</span>
    </button>
  );
}
