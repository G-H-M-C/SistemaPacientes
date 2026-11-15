import type { Cita, CitaInput } from '../types'
import { crearServicio } from './api'

export const citaService = crearServicio<Cita, CitaInput>('Citas')
