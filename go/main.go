package main

import (
	"crypto/tls"
	"crypto/x509"
	"fmt"
	"io/ioutil"
	"log"
	"net/http"
	"time"

	"golang.org/x/net/http2"
)

func main() {
	clientCertFile := "client.pem"
	clientKeyFile := "client.key"
	caCertFile := "ca.pem"
	var cert tls.Certificate
	var err error
	if clientCertFile != "" && clientKeyFile != "" {
		cert, err = tls.LoadX509KeyPair(clientCertFile, clientKeyFile)
		if err != nil {
			fmt.Println(err)
			log.Fatalf("Error creating x509 keypair from client cert file %s and client key file %s", clientCertFile, clientKeyFile)
		}
	}
	caCert, err := ioutil.ReadFile(caCertFile)
	if err != nil {
		fmt.Printf("Error opening cert file %s, Error: %s", caCertFile, err)
	}
	caCertPool := x509.NewCertPool()
	caCertPool.AppendCertsFromPEM(caCert)
	t := &http2.Transport{
		TLSClientConfig: &tls.Config{
			Certificates: []tls.Certificate{cert},
			RootCAs:      caCertPool,
		},
	}

	client := http.Client{Transport: t, Timeout: 15 * time.Second}
	resp, err := client.Get("https://localhost:5001/WeatherForecast")
	if err != nil {
		fmt.Printf("Failed get: %s\r\n", err)
	}
	defer resp.Body.Close()
	body, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		fmt.Printf("Failed reading response body: %s\r\n", err)
	}
	fmt.Printf("Client Got response %d: %s %s\r\n", resp.StatusCode, resp.Proto, string(body))

}
