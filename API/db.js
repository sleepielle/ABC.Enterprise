import {createPool} from 'mysql2/promise';

export const employees = createPool({
    host:'localhost',
    user: 'root',
    password: 'mimic3',
    port:3306,
    database:'employees'
})

export const cars = createPool({
    host:'localhost',
    user: 'root',
    password: 'mimic3',
    port:3306,
    database:'cars'
})

export const branches = createPool({
    host:'localhost',
    user: 'root',
    password: 'mimic3',
    port:3306,
    database:'branches'
})

