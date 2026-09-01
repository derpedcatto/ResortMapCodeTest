import classNames from "classnames";
import type { ComponentProps } from "react";
import styles from "./Button.module.scss";

type ButtonVariant = "primary" | "secondary";

type ButtonProps = ComponentProps<"button"> & {
  variant?: ButtonVariant;
};

export function Button({
  variant = "primary",
  type = "button",
  className,
  ...props
}: ButtonProps) {
  return (
    <button
      type={type}
      className={classNames(styles.button, styles[variant], className)}
      {...props}
    />
  );
}
