import type { ButtonHTMLAttributes } from 'react'

export type ButtonVariant = 'primary' | 'secondary'

export interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: ButtonVariant
}

/**
 * Placeholder primitive establishing the pattern: apps consume shared
 * components from design-system rather than each defining their own
 * button styling. Real primitives/tokens are a Wave 2 (Phase 30) design
 * decision — see docs/frontend/react-architecture.md.
 */
export function Button({ variant = 'primary', className, ...rest }: ButtonProps) {
  return <button className={['itsm-button', `itsm-button--${variant}`, className].filter(Boolean).join(' ')} {...rest} />
}
