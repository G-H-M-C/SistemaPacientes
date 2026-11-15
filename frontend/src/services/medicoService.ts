import type { Medico, MedicoInput } from '../types'
import { crearServicio } from './api'

export const medicoService = crearServicio<Medico, MedicoInput>('Medicos')
