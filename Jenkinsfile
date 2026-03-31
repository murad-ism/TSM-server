pipeline {
    agent any

    environment {
        APP_NAME = 'tsm-server-api'
        DOCKERHUB_REPO = 'a2x4ag9ahrcve6mt/tsm-api'
        IMAGE_TAG = "${BUILD_NUMBER}"
        FULL_IMAGE = "${DOCKERHUB_REPO}:${IMAGE_TAG}"

        // Jenkins credentials IDs
        DOCKERHUB_CREDENTIALS_ID = 'dockerhub-credentials'
        KUBECONFIG_CREDENTIALS_ID = 'kubeconfig'
    }

    options {
        timestamps()
    }

    stages {
        stage('Source') {
            steps {
                checkout scm
                sh '''
                    git fetch origin develop
                    git checkout develop || git checkout -b develop origin/develop
                    git pull origin develop
                '''
            }
        }

        stage('Build') {
            steps {
                sh '''
                    docker build \
                      -f TradingSystemsMonitoring.RestAPI/Dockerfile \
                      -t ${FULL_IMAGE} \
                      .
                '''
            }
        }

        stage('Test') {
            steps {
                sh '''
                    docker compose up test-runner --build --abort-on-container-exit --exit-code-from test-runner
                '''
            }
            post {
                always {
                    sh '''
                        docker compose down -v --remove-orphans || true
                    '''
                }
            }
        }

        stage('Push image') {
            steps {
                withCredentials([usernamePassword(
                    credentialsId: "${DOCKERHUB_CREDENTIALS_ID}",
                    usernameVariable: 'DOCKERHUB_USERNAME',
                    passwordVariable: 'DOCKERHUB_PASSWORD'
                )]) {
                    sh '''
                        echo "${DOCKERHUB_PASSWORD}" | docker login -u "${DOCKERHUB_USERNAME}" --password-stdin
                        docker push ${FULL_IMAGE}
                        docker logout
                    '''
                }
            }
        }

        stage('Deploy') {
            steps {
                withCredentials([file(credentialsId: "${KUBECONFIG_CREDENTIALS_ID}", variable: 'KUBECONFIG_FILE')]) {
                    sh '''
                        export KUBECONFIG="${KUBECONFIG_FILE}"

                        kubectl apply -f - <<EOF
                        apiVersion: apps/v1
                        kind: Deployment
                        metadata:
                          name: ${APP_NAME}
                          labels:
                            app: ${APP_NAME}
                        spec:
                          replicas: 2
                          selector:
                            matchLabels:
                              app: ${APP_NAME}
                          template:
                            metadata:
                              labels:
                                app: ${APP_NAME}
                            spec:
                              containers:
                              - name: ${APP_NAME}
                                image: ${FULL_IMAGE}
                                imagePullPolicy: Always
                                ports:
                                - containerPort: 8080
                        ---
                        apiVersion: v1
                        kind: Service
                        metadata:
                          name: ${APP_NAME}
                          labels:
                            app: ${APP_NAME}
                        spec:
                          selector:
                            app: ${APP_NAME}
                          ports:
                          - protocol: TCP
                            port: 80
                            targetPort: 8080
                          type: ClusterIP
                        EOF
                    '''
                }
            }
        }

        stage('Verify deployment') {
            steps {
                withCredentials([file(credentialsId: "${KUBECONFIG_CREDENTIALS_ID}", variable: 'KUBECONFIG_FILE')]) {
                    sh '''
                        export KUBECONFIG="${KUBECONFIG_FILE}"

                        kubectl rollout status deployment/${APP_NAME} --timeout=180s
                        kubectl get pods -l app=${APP_NAME}
                        kubectl get svc ${APP_NAME}

                        READY_REPLICAS=$(kubectl get deployment ${APP_NAME} -o jsonpath='{.status.readyReplicas}')
                        READY_REPLICAS=${READY_REPLICAS:-0}
                        if [ "${READY_REPLICAS}" -lt 2 ]; then
                          echo "Expected at least 2 ready replicas, got ${READY_REPLICAS}"
                          exit 1
                        fi
                    '''
                }
            }
        }
    }
}
