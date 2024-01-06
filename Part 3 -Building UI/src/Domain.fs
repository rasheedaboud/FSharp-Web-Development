module Domain

module Temperature =

    let toCelsius (temp: float) : float = (temp - 32.0) * 5.0 / 9.0

    let toFahrenheit (temp: float) : float = (temp * 9.0 / 5.0) + 32.0
