import type { Paciente, PacienteInput } from '../types'
import { crearServicio } from './api'

export const pacienteService = crearServicio<Paciente, PacienteInput>('Pacientes')
